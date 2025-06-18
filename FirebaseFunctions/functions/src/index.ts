/**
 * Import function triggers from their respective submodules:
 *
 * import {onCall} from "firebase-functions/v2/https";
 * import {onDocumentWritten} from "firebase-functions/v2/firestore";
 *
 * See a full list of supported triggers at https://firebase.google.com/docs/functions
 */

import * as functions from "firebase-functions/v1";
import {onRequest, onCall, HttpsError} from "firebase-functions/v2/https";
import * as admin from "firebase-admin";
import {UserData} from "./types/user";

admin.initializeApp();

export const helloWorld = onRequest((request, response) => {
  response.send("Hello from Firebase!");
});

// Firestore 테스트 함수
export const testFirestore = onCall(async (request) => {
  if (!request.auth) {
    throw new HttpsError("unauthenticated", "인증이 필요합니다.");
  }

  try {
    const testRef = admin.firestore().collection("test").doc(request.auth.uid);
    await testRef.set({
      message: "테스트 메시지",
      timestamp: admin.firestore.FieldValue.serverTimestamp(),
    });

    return {success: true, message: "Firestore 테스트 성공"};
  } catch (error) {
    throw new HttpsError("internal", "Firestore 테스트 실패");
  }
});

// --- 계정 정보 조회 함수 ---
// Firebase SDK를 통해 호출되며, 인증 정보를 자동으로 받습니다.
export const getUserAccountInfo =
onCall(async (request): Promise<UserData | null> => {
  if (!request.auth) {
    // HttpsError 사용 시, 클라이언트에 명확한 오류 코드와 메시지를 전달할 수 있습니다.
    throw new HttpsError(
      "unauthenticated",
      "계정 정보를 조회하려면 로그인이 필요합니다."
    );
  }

  const uid = request.auth.uid;
  console.log(`Attempting to fetch user document for uid: ${uid}`);

  try {
    // 2. Firestore에서 사용자 문서 조회
    const userDocRef = admin.firestore().collection("users").doc(uid);
    const userDoc = await userDocRef.get();

    // 3. 문서 존재 여부 확인 및 데이터 반환
    if (!userDoc.exists) {
      console.warn(`User document not found for uid: ${uid}`);
      // 문서가 없으면 404 not-found 에러 발생
      throw new HttpsError(
        "not-found",
        "사용자 계정 정보가 Firestore에 없습니다."
      );
    }

    console.log(`Successfully fetched user document for uid: ${uid}`);

    // 4. UserData interface에 맞는 데이터만 반환
    const userData = userDoc.data() as UserData;

    // onCall은 Timestamp를 Date 객체로 자동 변환해줍니다. 별도 변환 필요 없음.
    return userData;
  } catch (error: any) {
    console.error(`Error fetching user document for uid: ${uid}`, error);
    // 이미 HttpsError인 경우 그대로 throw하고, 아닌 경우 internal 에러로 래핑합니다.
    if (error instanceof HttpsError) {
      throw error;
    }
    throw new HttpsError(
      "internal",
      "계정 정보를 조회하는 중 오류가 발생했습니다.",
      error.message
    );
  }
});

// --- 계정 생성 시 UserData 문서 생성 트리거 ---
export const createUserDocument =
functions.auth.user().onCreate(async (user: admin.auth.UserRecord) => {
  const uid = user.uid;
  const displayName = user.displayName;

  console.log(`User created: ${uid}`);

  try {
    const userDocRef = admin.firestore().collection("users").doc(uid);

    const userDoc = await userDocRef.get();
    if (userDoc.exists) {
      console.warn(`User document already exists ${uid}.`);
      return null;
    }

    const initialUserData: UserData = {
      uid: uid,
      nickname: displayName || `user_${uid.substring(0, 6)}`,
      createdAt: admin.firestore.FieldValue
        .serverTimestamp() as admin.firestore.Timestamp,
      lastLoginAt: admin.firestore.FieldValue
        .serverTimestamp() as admin.firestore.Timestamp,
    };

    await userDocRef.set(initialUserData);
    console.log(`Created user document for uid: ${uid}`);
    return {success: true, uid: uid};
  } catch (error) {
    console.error(`Error creating user document for uid: ${uid}`, error);
    // In a background trigger like onCreate, you typically log the error
    // instead of throwing an HttpsError back to a client.
    throw new Error(`Failed to create user document: ${error}`);
  }
});
