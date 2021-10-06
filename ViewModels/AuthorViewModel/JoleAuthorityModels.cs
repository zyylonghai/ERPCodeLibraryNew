using ErpModels.Author;
using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModels.AuthorViewModel
{
    public class JoleAuthorityModels
    {
        public List<Jole> Joles { get; set; }

        public List<JoleD> JoleDs { get; set; }
        public List<AuthorityObj> AuthorityObjs { get; set; }
    }
}
