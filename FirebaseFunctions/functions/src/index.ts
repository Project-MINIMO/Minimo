/**
 * Import function triggers from their respective submodules:
 *
 * import {onCall} from "firebase-functions/v2/https";
 * import {onDocumentWritten} from "firebase-functions/v2/firestore";
 *
 * See a full list of supported triggers at https://firebase.google.com/docs/functions
 */

import {onRequest, onCall} from "firebase-functions/v2/https";
import * as admin from "firebase-admin";

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
    throw new Error("Firestore 테스트 실패");
  }
});
