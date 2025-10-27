using System;
using Bogus;

namespace Doera.Tests.TestSupport {

    public abstract class BaseTest {
        static BaseTest() {
            Randomizer.Seed = new Random(420);
        }

        protected static string[] SplitTags(string? s) =>
            string.IsNullOrWhiteSpace(s)
                ? []
                : s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}