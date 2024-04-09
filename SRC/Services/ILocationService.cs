﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallPaperGenerator.Models;

namespace WallPaperGenerator.Services
{
    public interface ILocationService
    {
        Task<LocationData> GetCurrentLocationAsync();
    }
}
