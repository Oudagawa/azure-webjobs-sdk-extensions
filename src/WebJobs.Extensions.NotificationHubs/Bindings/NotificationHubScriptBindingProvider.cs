﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Script.Extensibility;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Extensions.NotificationHubs
{
    /// <summary>
    /// BindingProvider for NotificationHub extensions
    /// </summary>
    public class NotificationHubScriptBindingProvider : ScriptBindingProvider
    {
        /// <inheritdoc/>
        public NotificationHubScriptBindingProvider(JobHostConfiguration config, JObject hostMetadata, TraceWriter traceWriter) 
            : base(config, hostMetadata, traceWriter)
        {
        }

        /// <inheritdoc/>
        public override bool TryCreate(ScriptBindingContext context, out ScriptBinding binding)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            binding = null;

            if (string.Compare(context.Type, "notificationHub", StringComparison.OrdinalIgnoreCase) == 0)
            {
                binding = new NotificationHubScriptBinding(context);
            }

            return binding != null;
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            Config.UseNotificationHubs();
        }

        /// <inheritdoc/>
        public override bool TryResolveAssembly(string assemblyName, out Assembly assembly)
        {
            assembly = null;

            if (string.Compare(assemblyName, "Microsoft.Azure.NotificationHubs", StringComparison.OrdinalIgnoreCase) == 0)
            {
                assembly = typeof(Notification).Assembly;
            }

            return assembly != null;
        }

        private class NotificationHubScriptBinding : ScriptBinding
        {
            public NotificationHubScriptBinding(ScriptBindingContext context) : base(context)
            {
            }

            public override Type DefaultType
            {
                get
                {
                    // Only output bindings are supported.
                    return typeof(IAsyncCollector<string>);
                }
            }

            public override Collection<Attribute> GetAttributes()
            {
                return new Collection<Attribute>
                {
                    new NotificationHubAttribute
                    {
                        TagExpression = Context.GetMetadataValue<string>("tagExpression"),
                        ConnectionStringSetting = Context.GetMetadataValue<string>("connection"),
                        HubName = Context.GetMetadataValue<string>("hubName"),
                        Platform = Context.GetMetadataEnumValue<NotificationPlatform>("platform")
                    }
                };
            }
        }
    }
}
