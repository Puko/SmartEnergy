using Microsoft.EntityFrameworkCore;
using SmartEnergy.Database.Models;
using SmartEnergy.Database.Repositories;

namespace SmartEnergy.Services
{
    public class UserService
    {
        private readonly UserInformationRepository _userRepository;
        private readonly SceneDeviceRepository _sceneDeviceRepository;

        public UserService(UserInformationRepository userRepository, SceneDeviceRepository sceneDeviceRepository)
        {
            _userRepository = userRepository;
            _sceneDeviceRepository = sceneDeviceRepository;
        }

        public void Logout()
        {
            var existingUser = _userRepository.GetSingle(null, x => x.Include(x => x.User).Include(x => x.Devices));
            if (existingUser != null)
            {
                _userRepository.Delete(existingUser);
                _userRepository.Save();
            }
        }

        public void Login(UserInformation user)
        {
            Logout();

            //refresh tokens to stored devices
            var storedSceneDevices = _sceneDeviceRepository.GetAll().ToList();
            foreach (var sceneDevice in storedSceneDevices) 
            {
                var device = user.Devices.FirstOrDefault(x => x.Mac.Equals(sceneDevice.Mac));
                if(device!= null) 
                {
                    sceneDevice.Token = device.Token;
                    _sceneDeviceRepository.Update(sceneDevice);
                }
            }

            _userRepository.Add(user);
            _userRepository.Save();
        }

        public IEnumerable<Database.Models.Device> GetUserData()
        {
            var existingUser = _userRepository.GetSingle(null, x => x.Include(x => x.User).Include(x => x.Devices));
            return existingUser.Devices;
        }

        public User GetUser()
        {
            var existingUser = _userRepository.GetSingle(null, x => x.Include(x => x.User).Include(x => x.Devices));
            return existingUser.User;
        }

        public bool IsLogged()
        {
            var existingUser = _userRepository.GetSingle();
            return existingUser != null;
        }
    }
}
