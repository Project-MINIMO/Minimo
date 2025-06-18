import * as admin from "firebase-admin";

export interface UserData {
  uid: string;
  nickname: string;
  createdAt: admin.firestore.Timestamp;
  lastLoginAt: admin.firestore.Timestamp;
}
