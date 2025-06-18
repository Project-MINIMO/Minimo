import * as admin from "firebase-admin";

export interface UserData {
  uid: string;
  nickname: string;
  createdAt: admin.firestore.Timestamp;
  lastLoginAt: admin.firestore.Timestamp;
  currencies: {
    SDC: number; // 별똥전 (StarDustCoin): 일반 재화
    SLP: number; // 별빛가루 (StarlightPowder): 유료 재화
    WSD: number; // 소원씨앗 (WishSeed): 소원빌기 전용 재화
    HDP: number; // 행복방울 (HeartDrop): 미니모 관련 재화
  };
}
