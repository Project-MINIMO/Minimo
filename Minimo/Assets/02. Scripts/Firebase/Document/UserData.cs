using System;
using System.Collections.Generic;
using Firebase.Firestore;

[FirestoreData]
public class UserData
{
    [FirestoreProperty]
    public string uid { get; set; }

    [FirestoreProperty]
    public string nickname { get; set; }

    [FirestoreProperty]
    public Timestamp createdAt { get; set; }

    [FirestoreProperty]
    public Timestamp lastLoginAt { get; set; }

    [FirestoreProperty]
    public Dictionary<string, long> currencies { get; set; } // SDC, SLP, WSD, HDP
}