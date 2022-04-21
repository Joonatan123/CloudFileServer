architecture = linux-x64
self-contained = false
release = Production


define pub =
	rm -rf builds/$(architecture)/*
	dotnet publish -c $(release) -r $(architecture) --self-contained $(self-contained) --output builds/$(architecture)
	cp InstallService.sh builds/$(architecture)
	cp CloudFileServer.service.template builds/$(architecture)
endef

x64:
	$(eval release = Development)
	$(call pub)
	rm builds/$(architecture)/appsettings.Production.json
	mv builds/$(architecture)/appsettings.Development.json builds/$(architecture)/appsettings.json
	mkdir builds/$(architecture)/cert
	cp cert/cert.pfx builds/$(architecture)/cert
arm :
	set -e
	$(eval architecture = linux-arm)
	$(eval self-contained = true)
	@echo $(architecture)
	$(call pub)
	rm builds/$(architecture)/appsettings.Development.json
	mv builds/$(architecture)/appsettings.Production.json builds/$(architecture)/appsettings.json
	mkdir builds/$(architecture)/cert
	cp cert/cert1.pem cert/privkey1.pem builds/$(architecture)/cert


