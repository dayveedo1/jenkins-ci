using EazyMobile.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewDevice
    {
        public string UserId { get; set; }
        public string DeviceId { get; set; }

        public static implicit operator ViewDevice(Device device)
        {
            return new ViewDevice
            {
                UserId = device.UserId,
                DeviceId = device.DeviceId
            };
        }

        public static implicit operator Device(ViewDevice device)
        {
            return new Device
            {
                UserId = device.UserId,
                DeviceId = device.DeviceId
            };
        }
    }
}
