using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CloudFileServer.Models
{
    public class SortInfo
    {
        public string SortBy = "";
        public bool Reverse = false;
        public bool GetNotReverse(string name)
        {
            if(SortBy.Equals(name))
                return !Reverse;
            return false;
        }
    }
}