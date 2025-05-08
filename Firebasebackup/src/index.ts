import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';

admin.initializeApp();

// Firestore 데이터베이스 참조
const db = admin.firestore();

// 사용자 점수 업데이트 함수의 파라미터 타입
interface UpdateScoreData {
    score: number;
}

// 사용자 랭킹 정보 타입
interface UserRanking {
    userId: string;
    score: number;
    displayName: string;
}

// 응답 타입
interface FunctionResponse<T> {
    success: boolean;
    error?: string;
    data?: T;
}

// 사용자 점수 업데이트 함수
export const updateUserScore = functions.https.onCall(async (data: UpdateScoreData, context): Promise<FunctionResponse<{ newScore: number }>> => {
    // 인증 확인
    if (!context.auth) {
        throw new functions.https.HttpsError('unauthenticated', '로그인이 필요합니다.');
    }

    const { score } = data;
    const userId = context.auth.uid;

    try {
        // 사용자 문서 참조
        const userRef = db.collection('users').doc(userId);
        
        // 현재 점수 가져오기
        const userDoc = await userRef.get();
        const currentScore = userDoc.exists ? (userDoc.data()?.score || 0) : 0;
        
        // 새 점수 계산 (현재 점수 + 추가 점수)
        const newScore = currentScore + score;
        
        // 점수 업데이트
        await userRef.set({
            score: newScore,
            lastUpdated: admin.firestore.FieldValue.serverTimestamp()
        }, { merge: true });

        return { 
            success: true,
            data: { newScore }
        };
    } catch (error) {
        throw new functions.https.HttpsError('internal', '점수 업데이트 중 오류가 발생했습니다.');
    }
});

// 사용자 랭킹 조회 함수
export const getUserRanking = functions.https.onCall(async (_, context): Promise<FunctionResponse<UserRanking[]>> => {
    // 인증 확인
    if (!context.auth) {
        throw new functions.https.HttpsError('unauthenticated', '로그인이 필요합니다.');
    }

    try {
        // 상위 10명의 사용자 점수 조회
        const snapshot = await db.collection('users')
            .orderBy('score', 'desc')
            .limit(10)
            .get();

        const rankings: UserRanking[] = [];
        snapshot.forEach(doc => {
            const data = doc.data();
            rankings.push({
                userId: doc.id,
                score: data.score || 0,
                displayName: data.displayName || 'Unknown'
            });
        });

        return { 
            success: true,
            data: rankings
        };
    } catch (error) {
        throw new functions.https.HttpsError('internal', '랭킹 조회 중 오류가 발생했습니다.');
    }
}); 