using Microsoft.EntityFrameworkCore;
using SmartEnergy.Database.Models;
using SmartEnergy.Database.Repositories;

namespace SmartEnergy.Services
{
    public class SceneService
    {
        private readonly SceneRepository _sceneRepository;
        private readonly SceneDeviceRepository _sceneDeviceRepository;

        public SceneService(SceneRepository sceneRepository, SceneDeviceRepository sceneDeviceRepository)
        {
            _sceneRepository = sceneRepository;
            _sceneDeviceRepository = sceneDeviceRepository;
        }

        public IEnumerable<Scene> GetScenes()
        {
            return _sceneRepository.GetAll(null, x => x.Include(x => x.Devices))
                .Select(x => new Scene
                {
                    Id = x.Id,
                    Name = x.Name,
                    DevicesCount = x.Devices.Count
                });
        }

        public Scene GetSceneById(int id)
        {
            return _sceneRepository.GetSingle(x => x.Id == id, x => x.Include(y => y.Devices));
        }

        public void Add(Scene scene)
        {
            foreach (var item in scene.Devices)
            {
                _sceneRepository.Add(item);
            }

            _sceneRepository.Add(scene);
            _sceneRepository.Save();
        }

        public void Update(Scene scene)
        {
            _sceneRepository.RemoveFromChangeTracker(scene);

            foreach (var item in scene.Devices)
            {
                _sceneRepository.Update(item);
            }

            _sceneRepository.Update(scene);
            _sceneRepository.Save();
        }

        public void Delete(Scene scene)
        {
            _sceneRepository.RemoveFromChangeTracker(scene);

            foreach (var item in scene.Devices)
            {
                _sceneDeviceRepository.Delete(item);
            }

            _sceneRepository.Delete(scene);
            _sceneRepository.Save();
        }
    }
}
