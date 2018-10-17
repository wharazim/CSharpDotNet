using MvcPartialViews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPartialViews.ViewModels
{
    public class BigBadVoodooViewModel
    {
        IEnumerable<Residence> MyResidences;

        IEnumerable<Employer> MyEmployers;
    }
}