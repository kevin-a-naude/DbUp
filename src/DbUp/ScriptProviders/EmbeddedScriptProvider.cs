﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DbUp.ScriptProviders
{
    /// <summary>
    /// The default <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts embedded in an assembly.
    /// </summary>
    public sealed class EmbeddedScriptProvider : IScriptProvider
    {
        private readonly Assembly assembly;
        private readonly Func<string, bool> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public EmbeddedScriptProvider(Assembly assembly) : this(assembly, x => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The filter.</param>
        public EmbeddedScriptProvider(Assembly assembly, Func<string, bool> filter)
        {
            this.assembly = assembly;
            this.filter = filter;
        }

        public IEnumerable<SqlScript> GetScripts()
        {
            return assembly
                .GetManifestResourceNames()
                .Where(filter)
                .OrderBy(x => x)
                .Select(ReadResourceAsScript)
                .ToList();
        }

        private SqlScript ReadResourceAsScript(string scriptName)
        {
            string contents;
            var resourceStream = assembly.GetManifestResourceStream(scriptName);
            using (var resourceStreamReader = new StreamReader(resourceStream))
            {
                contents = resourceStreamReader.ReadToEnd();
            }

            return new SqlScript(scriptName, contents);
        }
    }
}