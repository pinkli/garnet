﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO;

namespace Garnet.server
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class GarnetObject
    {
        /// <summary>
        /// Create initial value of object
        /// </summary>
        /// <param name="garnetObjectType"></param>
        /// <returns></returns>
        internal static IGarnetObject Create(GarnetObjectType garnetObjectType)
        {
            return garnetObjectType switch
            {
                GarnetObjectType.SortedSet => new SortedSetObject(),
                GarnetObjectType.List => new ListObject(),
                GarnetObjectType.Hash => new HashObject(),
                GarnetObjectType.Set => new SetObject(),
                _ => throw new Exception("Unsupported data type"),
            };
        }

        /// <summary>
        /// Check if object creation is necessary
        /// </summary>
        /// <returns></returns>
        internal static bool NeedToCreate(RespInputHeader header)
        {
            return header.type switch
            {
                GarnetObjectType.SortedSet => header.SortedSetOp switch
                {
                    SortedSetOperation.ZREM => false,
                    SortedSetOperation.ZPOPMIN => false,
                    SortedSetOperation.ZPOPMAX => false,
                    SortedSetOperation.ZREMRANGEBYLEX => false,
                    SortedSetOperation.ZREMRANGEBYSCORE => false,
                    SortedSetOperation.ZREMRANGEBYRANK => false,
                    _ => true,
                },
                GarnetObjectType.List => header.ListOp switch
                {
                    ListOperation.LPOP => false,
                    ListOperation.RPOP => false,
                    ListOperation.LRANGE => false,
                    ListOperation.LINDEX => false,
                    ListOperation.LTRIM => false,
                    ListOperation.LREM => false,
                    ListOperation.LINSERT => false,
                    ListOperation.LPUSHX => false,
                    ListOperation.RPUSHX => false,
                    _ => true,
                },
                GarnetObjectType.Set => header.SetOp switch
                {
                    SetOperation.SCARD => false,
                    SetOperation.SMEMBERS => false,
                    SetOperation.SREM => false,
                    SetOperation.SPOP => false,
                    _ => true,
                },
                GarnetObjectType.Expire => false,
                GarnetObjectType.Persist => false,
                _ => true,
            };
        }

        /// <summary>
        /// Create an IGarnetObject from an input array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IGarnetObject Create(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);
            var type = (GarnetObjectType)reader.ReadByte();

            return type switch
            {
                GarnetObjectType.SortedSet => new SortedSetObject(reader),
                GarnetObjectType.List => new ListObject(reader),
                GarnetObjectType.Hash => new HashObject(reader),
                GarnetObjectType.Set => new SetObject(reader),
                _ => throw new Exception("Unsupported data type"),
            };
        }
    }
}