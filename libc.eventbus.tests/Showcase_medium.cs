using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using libc.eventbus.System;
using libc.eventbus.Types;

namespace libc.eventbus.tests {

    [TestClass]
    public class Showcase_medium {
        [TestMethod]
        public void Showcase() {
            // 1- create an event bus
            var bus = new DefaultEventBus();

            // 2- create services
            var userService = new UserService(bus);
            var resumeService = new ResumeService();
            var storeService = new StoreService();

            // 3- subscribe
            bus.Subscribe<MaritalStatusChanged, ResumeService.MaritalStatusChangedHandler>(
                new ResumeService.MaritalStatusChangedHandler(resumeService));
            bus.Subscribe<MaritalStatusChanged, StoreService.MaritalStatusChangedHandler>(
                new StoreService.MaritalStatusChangedHandler(storeService));

            // 4- someone got married
            userService.GotMarried("1");
        }

        public class UserService {
            private readonly ICollection<User> db = new List<User> {
                new User {
                    Id = "1",
                    IsMarried = false
                }
            };

            private readonly IEventEmitter eventEmitter;
            public UserService(IEventEmitter eventEmitter) {
                this.eventEmitter = eventEmitter;
            }

            public void GotMarried(string userId) {
                var user = db.First(a => a.Id.Equals(userId));
                user.IsMarried = true;

                // propagate changes to other parts of the code
                eventEmitter.Publish(new MaritalStatusChanged(userId, true));
            }
        }

        public class ResumeService {
            private readonly ICollection<Resume> db = new List<Resume> {
                new Resume {
                    Description = "My current resume",
                    UserId = "1",
                    IsUserMarried = false
                }
            };

            public void SetMaritalStatus(string userId, bool isMarried) {
                foreach (var resume in db.Where(a => a.UserId.Equals(userId))) {
                    resume.IsUserMarried = isMarried;
                }

                Console.WriteLine($"{userId} is {(isMarried ? "married" : "single")} now");
            }

            public class MaritalStatusChangedHandler : IEventHandler<MaritalStatusChanged> {
                private readonly ResumeService service;
                public MaritalStatusChangedHandler(ResumeService service) {
                    this.service = service;
                }

                public Task Handle(MaritalStatusChanged ev) {
                    service.SetMaritalStatus(ev.UserId, ev.IsMarried);

                    return Task.CompletedTask;
                }
            }
        }

        public class StoreService {
            private readonly ICollection<Store> db = new List<Store> {
                new Store {
                    Title = "Restaurant",
                    OwnerId = "1",
                    IsOwnerMarried = false
                }
            };

            public void SetMaritalStatus(string userId, bool isMarried) {
                foreach (var store in db.Where(a => a.OwnerId.Equals(userId))) {
                    store.IsOwnerMarried = isMarried;
                }

                Console.WriteLine($"{userId} is {(isMarried ? "married" : "single")} now");
            }

            public class MaritalStatusChangedHandler : IEventHandler<MaritalStatusChanged> {
                private readonly StoreService service;
                public MaritalStatusChangedHandler(StoreService service) {
                    this.service = service;
                }

                public Task Handle(MaritalStatusChanged ev) {
                    service.SetMaritalStatus(ev.UserId, ev.IsMarried);

                    return Task.CompletedTask;
                }
            }
        }

        public class MaritalStatusChanged : IEvent {
            public MaritalStatusChanged(string userId, bool isMarried) {
                UserId = userId;
                IsMarried = isMarried;
            }
            public string UserId { get; }
            public bool IsMarried { get; }
        }

        public class User {
            public string Id { get; set; }
            public bool IsMarried { get; set; }
        }

        public class Resume {
            public string Description { get; set; }
            public string UserId { get; set; }
            public bool IsUserMarried { get; set; }
        }

        public class Store {
            public string Title { get; set; }
            public string OwnerId { get; set; }
            public bool IsOwnerMarried { get; set; }
        }
    }

}