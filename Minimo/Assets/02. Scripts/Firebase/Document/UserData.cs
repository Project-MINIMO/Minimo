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
    public Currency currencies { get; set; }

    [FirestoreData]
    public class Currency
    {
        [FirestoreProperty]
        public int SDC { get; set; } // 별똥전

        [FirestoreProperty]
        public int SLP { get; set; } // 별빛가루

        [FirestoreProperty]
        public int WSD { get; set; } // 소원씨앗

        [FirestoreProperty]
        public int HDP { get; set; } // 행복방울
        
        public override string ToString()
        {
            return $"Currency(SDC: {SDC}, SLP: {SLP}, WSD: {WSD}, HDP: {HDP})";
        }
    }
    
    public override string ToString()
    {
        return $"UserData(uid: {uid}, nickname: {nickname}, createdAt: {createdAt.ToDateTime()}, lastLoginAt: {lastLoginAt.ToDateTime()}, currencies: {currencies.ToString()})";
    }
}