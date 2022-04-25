architecture = linux-x64
self-contained = false
release = Production
remove = appsettings.Development.json,appsettings.Production.json


define pub =
	rm -rf builds/$(architecture)/*
	dotnet publish -c $(release) -r $(architecture) --self-contained $(self-contained) --output builds/$(architecture)
	cp InstallService.sh builds/$(architecture)
	cp Install.sh builds/$(architecture)
	cp CloudFileServer.service.template builds/$(architecture)
	rm builds/$(architecture)/appsettings.*.json
	cp appsettings.$(release).json.template builds/$(architecture)/appsettings.json.template
endef

x64:
	$(eval release = Development)
	$(call pub)
#	cp appsettings.Development.json.template builds/$(architecture)/appsettings.json.template
#	mv builds/$(architecture)/appsettings.Development.json.template builds/$(architecture)/appsettings.json.template
	mkdir builds/$(architecture)/cert
	cp cert/cert.pfx builds/$(architecture)/cert
arm :
	set -e
	$(eval architecture = linux-arm)
	$(eval self-contained = true)
	@echo $(architecture)
	$(call pub)
#	cp appsettings.Production.json.template builds/$(architecture)/appsettings.json.template
	mkdir builds/$(architecture)/cert
	cp cert/cert1.pem cert/privkey1.pem builds/$(architecture)/cert



zip :
	cd builds;\
	if [ -f "$(architecture).zip" ]; then\
		rm "$(architecture).zip";\
	fi;\
	zip -9 -r $(architecture).zip -r $(architecture);\

test:
	echo $(remove)