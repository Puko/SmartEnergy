using SmartEnergy.Database.Models;
using SmartEnergy.Database.Repositories;

namespace SmartEnergy.Services
{
    public class SceneDeviceService
    {
        private readonly SceneDeviceRepository _sceneDeviceRepository;

        public SceneDeviceService(SceneDeviceRepository sceneDeviceRepository)
        {
            _sceneDeviceRepository = sceneDeviceRepository;
        }

        public void Update(SceneDevice sceneDevice)
        {
            _sceneDeviceRepository.Update(sceneDevice);
            _sceneDeviceRepository.Save();
        }
    }
}
