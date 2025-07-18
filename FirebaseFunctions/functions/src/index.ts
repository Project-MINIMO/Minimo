/**
 * Import function triggers from their respective submodules:
 *
 * import {onCall} from "firebase-functions/v2/https";
 * import {onDocumentWritten} from "firebase-functions/v2/firestore";
 *
 * See a full list of supported triggers at https://firebase.google.com/docs/functions
 */

import * as functions from "firebase-functions/v1";
import { onCall, HttpsError } from "firebase-functions/v2/https";
import * as admin from "firebase-admin";
import { UserData } from "./types/user";

admin.initializeApp();

export const helloWorld = onCall(
  {
    region: "asia-northeast3",
  },
  (_data, _context) => {
    return { message: "Hello from Firebase! - onCall" };
  });

// Firestore 테스트 함수
export const testFirestore = onCall(
  {
    region: "asia-northeast3",
  },
  async (request) => {
    if (!request.auth) {
      throw new HttpsError("unauthenticated", "인증이 필요합니다.");
    }

    try {
      const testRef = admin.firestore().collection("test").doc(request.auth.uid);
      console.log("testRef path:", testRef.path); // 경로를 로깅하는 것이 더 유용합니다.

      await testRef.set({
        message: "테스트 메시지",
        timestamp: admin.firestore.FieldValue.serverTimestamp(),
      });

      console.log("Firestore write successful for UID:", request.auth.uid);

      return { success: true, message: "Firestore 테스트 성공" };
    } catch (error) { // 여기서 'error'는 'unknown' 타입입니다.
      console.error("Firestore 쓰기 실패:", error);

      let errorMessage = "Firestore 테스트 중 알 수 없는 오류가 발생했습니다.";

      // 타입을 확인하여 안전하게 message 속성에 접근합니다.
      if (error instanceof Error) {
        errorMessage = error.message;
      }

      throw new HttpsError(
        "internal",
        "Firestore 테스트 실패: " + errorMessage,
        error // 세 번째 인자로 원본 에러를 전달할 수 있습니다.
      );
    }
  }
);

// --- 계정 정보 조회 함수 ---
// Firebase SDK를 통해 호출되며, 인증 정보를 자동으로 받습니다.
export const getUserAccountInfo =
onCall(
  {
    region: "asia-northeast3",
  },
  async (request): Promise<UserData | null> => {
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
export const createUserAccount = onCall(
  { region: "asia-northeast3" },
  async (request) => {
    const uid = request.auth?.uid;
    if (!uid) {
      throw new HttpsError("unauthenticated", "로그인이 필요합니다.");
    }

    const userDocRef = admin.firestore().collection("users").doc(uid);
    const userDoc = await userDocRef.get();

    if (userDoc.exists) {
      throw new HttpsError("already-exists", "이미 계정이 생성되었습니다.");
    }

    const nickname = request.data.nickname || `user_${uid.substring(0, 6)}`;

    const userData: UserData = {
      uid,
      nickname,
      createdAt: admin.firestore.FieldValue.serverTimestamp() as any,
      lastLoginAt: admin.firestore.FieldValue.serverTimestamp() as any,
      currencies: { SDC: 0, SLP: 0, WSD: 0, HDP: 0 },
    };

    await userDocRef.set(userData);
    return { success: true };
  }
);


// --- 계정 삭제 시 UserData 문서 및 서브컬렉션 삭제 ---
export const deleteUserDocument = functions
  .region("asia-northeast3")
  .auth.user().onDelete(
    async (user) => {
      const uid = user.uid;
      const firestore = admin.firestore();
      const userDocRef = firestore.doc(`users/${uid}`);

      console.log(`Deleting user document and subcollections for uid: ${uid}`);

      try {
        // Firebase Admin SDK의 recursiveDelete 사용
        await firestore.recursiveDelete(userDocRef);

        console.log(`Successfully deleted user document and subcollections for uid: ${uid}`);
        return { success: true };
      } catch (error) {
        console.error(`Failed to delete user document for uid: ${uid}`, error);
        return null;
      }
    });
