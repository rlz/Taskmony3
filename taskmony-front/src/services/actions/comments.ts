import { Dispatch } from "redux";
import { checkResponse } from "../../utils/APIUtils";
import { getCookie } from "../../utils/cookies";
import { BASE_URL } from "../../utils/data";
import { useAppSelector } from "../../utils/hooks";

export const SEND_COMMENT_REQUEST = "SEND_COMMENT_REQUEST";
export const SEND_COMMENT_SUCCESS = "SEND_COMMENT_SUCCESS";
export const SEND_COMMENT_FAILED = "SEND_COMMENT_FAILED";

const URL = BASE_URL + "/graphql";

export function sendTaskComment(taskId: string, text: string) {
  //   console.log(`sending ${taskId}${text}`);
  return function (dispatch: Dispatch) {
    console.log("sending_comment");
    dispatch({ type: SEND_COMMENT_REQUEST });
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },

      body: JSON.stringify({
        query: `mutation {
          taskAddComment(taskId:"${taskId}", text:"${text}") {
            text
            createdAt
            createdBy { displayName }
        }
    }
        `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        console.log(res);
        if (res) {
          dispatch({
            type: SEND_COMMENT_SUCCESS,
            comment: res.data.taskAddComment,
            id: taskId,
          });
        } else {
          dispatch({
            type: SEND_COMMENT_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: SEND_COMMENT_FAILED,
        });
      });
  };
}

export function sendIdeaComment(ideaId: string, text: string) {
  //   console.log(`sending ${taskId}${text}`);
  return function (dispatch: Dispatch) {
    console.log("sending_comment");
    dispatch({ type: SEND_COMMENT_REQUEST });
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },

      body: JSON.stringify({
        query: `mutation {
          ideaAddComment(ideaId:"${ideaId}", text:"${text}") {
            text
            createdAt
            createdBy { displayName }
        }
    }
        `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        console.log(res);
        if (res) {
          dispatch({
            type: SEND_COMMENT_SUCCESS,
            comment: res.data.ideaAddComment,
            id: ideaId,
          });
        } else {
          console.log(res);
          dispatch({
            type: SEND_COMMENT_FAILED,
          });
        }
      })
      .catch((error) => {
        console.log(error);
        dispatch({
          type: SEND_COMMENT_FAILED,
        });
      });
  };
}
