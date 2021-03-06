﻿#region (c) 2010-2011 Lokad CQRS - New BSD License 

// Copyright (c) Lokad SAS 2010-2012 (http://www.lokad.com)
// This code is released as Open Source under the terms of the New BSD License
// Homepage: http://lokad.github.com/lokad-cqrs/

#endregion

using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Sample
{
    public static class IdentityConvert
    {
        public static IIdentity FromTransportable(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new InvalidOperationException();
            var args = value.Split(new[] {'-'}, 3);
            var id = args[2];
            var tag = int.Parse(args[0]);
            switch (tag)
            {
                case NullId.TagValue:
                    return NullId.Instance;
                case SecurityId.TagValue:
                    return new SecurityId(long.Parse(id));
                default:
                    var message = string.Format("Unknown identity tag {0} (name: {1})", tag, args[1]);
                    throw new InvalidOperationException(message);
            }
        }

        public static string ToStream(IIdentity identity)
        {
            return identity.Tag.ToString("000") + "-" + identity.GetId();
        }

        public static string ToTransportable(IIdentity identity)
        {
            var replace = identity.GetType().Name.Replace("Id", "").ToLowerInvariant();
            return string.Format("{0:000}-{1}-{2}", identity.Tag, replace, identity.GetId());
        }
    }

  

    [DataContract(Namespace = "Sample")]
    public sealed class SecurityId : AbstractIdentity<long>
    {
        public const int TagValue = 2;

        public SecurityId(long id)
        {
            Contract.Requires(id > 0);

            Id = id;
            Tag = TagValue;
        }

        [DataMember(Order = 2)]
        public override int Tag { get; protected set; }

        [DataMember(Order = 1)]
        public override long Id { get; protected set; }

        public SecurityId() {}
    }



    [DataContract(Namespace = "Sample")]
    public sealed class UserId : AbstractIdentity<long>
    {
        public const int TagValue = 3;

        public UserId(long id)
        {
            if (id <= 0)
                throw new InvalidOperationException("Tried to assemble non-existent login");
            Id = id;
            Tag = TagValue;
        }

        public UserId() {}

        [DataMember(Order = 2)]
        public override int Tag { get; protected set; }

        [DataMember(Order = 1)]
        public override long Id { get; protected set; }
    }

    [DataContract(Namespace = "Sample")]
    public sealed class RegistrationId : AbstractIdentity<Guid>
    {
        public const int TagValue = 4;

        public RegistrationId(Guid id)
        {
            Id = id;
            Tag = TagValue;
        }

        public RegistrationId() { }

        [DataMember(Order = 2)]
        public override int Tag { get; protected set; }

        [DataMember(Order = 1)]
        public override Guid Id { get; protected set; }

        public override string ToString()
        {
            return string.Format("reg-" + Id.ToString().ToLowerInvariant().Substring(0, 6));
        }

    }

}