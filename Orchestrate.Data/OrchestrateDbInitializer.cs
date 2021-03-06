﻿using Orchestrate.Data.Models;
using Orchestrate.Data.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orchestrate.Data.Models.Joins;

namespace Orchestrate.Data
{
    public class OrchestrateDbInitializer
    {
        private static readonly string[] NAMES = new[] { "One", "Two", "Three", "Four", "Five", "Six" };

        private OrchestrateContext _ctx;
        private readonly IStringHasher _stringHasher;

        public OrchestrateDbInitializer(OrchestrateContext ctx, IStringHasher stringHasher)
        {
            _ctx = ctx;
            _stringHasher = stringHasher;
        }

        public async Task Initialize()
        {
            var manager1 = new User
            {
                Email = $"manager_one@mail.com",
                PasswordHash = _stringHasher.Hash("one"),
                FirstName = "Manager",
                LastName = "One",
                IsPasswordTemporary = false
            };

            _ctx.Users.Add(manager1);

            var users = NAMES.Select(name => new User
            {
                Email = $"user_{name.ToLower()}@mail.com",
                PasswordHash = _stringHasher.Hash(name.ToLower()),
                FirstName = "User",
                LastName = name,
                IsPasswordTemporary = false
            }).ToArray();

            _ctx.Users.AddRange(users);

            await _ctx.SaveChangesAsync();

            var roles = new Role[]
            {
                new Role { Section = "Trombone", Num = 1 },
                new Role { Section = "Piano" },
                new Role { Section = "Trumpet", Num = 1 }
            };

            _ctx.Roles.AddRange(roles);
            await _ctx.SaveChangesAsync();

            var group1 = new Group
            {
                Name = "Group One",
                Manager = manager1,
                Directors = new List<User> { users[0], users[1] },
                Roles = new List<GroupRole>()
            };

            _ctx.Groups.Add(group1);
            await _ctx.SaveChangesAsync();

            var group2 = new Group
            {
                Name = "Group Two",
                Manager = users[0],
                Directors = new List<User> { users[2], users[3] },
                Roles = new List<GroupRole>()
            };

            _ctx.Groups.Add(group2);
            await _ctx.SaveChangesAsync();

            group1.Roles.Add(new GroupRole { Role = roles[0], Members = new List<User> { users[0], users[4] } });
            group1.Roles.Add(new GroupRole { Role = roles[1], Members = new List<User> { users[2] } });
            group1.Roles.Add(new GroupRole { Role = roles[2] });

            group2.Roles.Add(new GroupRole { Role = roles[1], Members = new List<User> { users[5] } });
            group2.Roles.Add(new GroupRole { Role = roles[2], Members = new List<User> { users[1] } });

            await _ctx.SaveChangesAsync();
        }
    }
}
