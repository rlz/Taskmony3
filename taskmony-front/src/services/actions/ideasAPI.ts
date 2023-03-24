import { Dispatch } from "redux";
import { checkResponse, getAccessToken, nowDate } from "../../utils/APIUtils";
import Cookies from 'js-cookie';
import { BASE_URL } from "../../utils/data";
import { useAppSelector } from "../../utils/hooks";
import { ideasAllQuery } from "../../utils/queries";
import { TDirection, TIdea } from "../../utils/types";
export const GET_IDEAS_REQUEST = "GET_IDEAS_REQUEST";
export const GET_IDEAS_SUCCESS = "GET_IDEAS_SUCCESS";
export const GET_IDEAS_FAILED = "GET_IDEAS_FAILED";
export const ADD_IDEA_REQUEST = "ADD_IDEA_REQUEST";
export const ADD_IDEA_SUCCESS = "ADD_IDEA_SUCCESS";
export const ADD_IDEA_FAILED = "ADD_IDEA_FAILED";
export const DELETE_IDEA_REQUEST = "DELETE_IDEA_REQUEST";
export const DELETE_IDEA_SUCCESS = "DELETE_IDEA_SUCCESS";
export const DELETE_IDEA_FAILED = "DELETE_IDEA_FAILED";

export const CHANGE_COMPLETE_IDEA_DATE_REQUEST =
  "CHANGE_COMPLETE_IDEA_DATE_REQUEST";
export const CHANGE_COMPLETE_IDEA_DATE_SUCCESS =
  "CHANGE_COMPLETE_IDEA_DATE_SUCCESS";
export const CHANGE_COMPLETE_IDEA_DATE_FAILED =
  "CHANGE_COMPLETE_IDEA_DATE_FAILED";
export const CHANGE_IDEA_FOLLOWED_REQUEST = "CHANGE_IDEA_FOLLOWED_REQUEST";
export const CHANGE_IDEA_FOLLOWED_SUCCESS = "CHANGE_IDEA_FOLLOWED_SUCCESS";
export const CHANGE_IDEA_FOLLOWED_FAILED = "CHANGE_IDEA_FOLLOWED_FAILED";

export const CHANGE_OPEN_IDEA = "CHANGE_OPEN_IDEA";

export const CHANGE_IDEA_DESCRIPTION = "CHANGE_IDEA_DESCRIPTION";
export const CHANGE_IDEA_DIRECTION = "CHANGE_IDEA_DIRECTION";
export const CHANGE_IDEA_GENERATION = "CHANGE_IDEA_GENERATION";
export const CHANGE_IDEA_DETAILS = "CHANGE_IDEA_DETAILS";
export const CHANGE_IDEA_REVIEWED_DATE = "CHANGE_IDEA_REVIEWED_DATE";

export const CHANGE_IDEA_DESCRIPTION_FAILED = "CHANGE_IDEA_DESCRIPTION_FAILED";
export const CHANGE_IDEA_DIRECTION_FAILED = "CHANGE_IDEA_DIRECTION_FAILED";
export const CHANGE_IDEA_GENERATION_FAILED = "CHANGE_IDEA_GENERATION_FAILED";
export const CHANGE_IDEA_DETAILS_FAILED = "CHANGE_IDEA_DETAILS_FAILED";
export const CHANGE_IDEA_REVIEWED_DATE_FAILED = "CHANGE_IDEA_REVIEWED_DATE_FAILED";


export const CHANGE_IDEA_DESCRIPTION_SUCCESS =
  "CHANGE_IDEA_DESCRIPTION_SUCCESS";
export const CHANGE_IDEA_DIRECTION_SUCCESS = "CHANGE_IDEA_DIRECTION_SUCCESS";
export const CHANGE_IDEA_GENERATION_SUCCESS = "CHANGE_IDEA_GENERATION_SUCCESS";
export const CHANGE_IDEA_DETAILS_SUCCESS = "CHANGE_IDEA_DETAILS_SUCCESS";
export const CHANGE_IDEA_REVIEWED_DATE_SUCCESS = "CHANGE_IDEA_REVIEWED_DATE_SUCCESS";


export const CHANGE_SAVED = "CHANGE_SAVED";

export const RESET_IDEA = "RESET_IDEA";
export const CHANGE_IDEAS = "CHANGE_IDEAS";

const URL = BASE_URL + "/graphql";

export function openIdea(id: string) {
  const ideas = useAppSelector((store) => store.ideas.items);
  const idea = ideas.filter((idea) => idea.id == id)[0];
  return function (dispatch: Dispatch) {
    dispatch({ type: GET_IDEAS_REQUEST, idea: idea });
  };
}

export function getIdeas() {
  return function (dispatch: Dispatch) {
    console.log("getting ideas");
    dispatch({ type: GET_IDEAS_REQUEST });
    getAccessToken().then((cookie)=>fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + cookie,
      },

      body: JSON.stringify({
        query: `{ideas{${ideasAllQuery}}}`,
      }),
    }))
      .then(checkResponse)
      .then((res) => {
        console.log(res);
        if (res) {
          dispatch({
            type: GET_IDEAS_SUCCESS,
            items: res.data.ideas,
          });
        } else {
          dispatch({
            type: GET_IDEAS_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: GET_IDEAS_FAILED,
        });
      });
  };
}

export function addIdea(idea : TIdea, direction: string | null) {
  return function (dispatch: Dispatch) {
    dispatch({ type: ADD_IDEA_REQUEST });
    console.log("adding");
   getAccessToken().then((cookie)=> fetch(URL, {
      method: "POST",

      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + Cookies.get("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      ideaAdd(description:"${idea.description}"
      ${
        direction
          ? `,directionId:"${direction}"`
          : idea.direction?.id
          ? `,directionId:"${idea.direction?.id}"`
          : ""
      }
      , generation: ${idea.generation ? idea.generation.toUpperCase().replaceAll(' ', '_') : "HOT"}
      ) {
        ${ideasAllQuery}
      }
    }
    `,
      }),
    }))
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: ADD_IDEA_SUCCESS,
            idea: res.data.ideaAdd,
          });
        } else {
          dispatch({
            type: ADD_IDEA_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: ADD_IDEA_FAILED,
        });
      });
  };
}

export function changeIdeaFollowed(ideaId: string, markFollowed : boolean) {
  return function (dispatch: Dispatch) {
    dispatch({ type: CHANGE_IDEA_FOLLOWED_REQUEST });
    console.log("change followed");
   getAccessToken().then((cookie)=> fetch(URL, {
      method: "POST",

      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + Cookies.get("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      ${markFollowed ? "ideaSubscribe" : "ideaUnsubscribe"}(ideaId:"${ideaId}") 
    }
    `,
      }),
    }))
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_IDEA_FOLLOWED_SUCCESS,
            ideaId: ideaId,
            followed: markFollowed,
            userId: Cookies.get("id"),
          });
        } else {
          dispatch({
            type: CHANGE_IDEA_FOLLOWED_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_IDEA_FOLLOWED_FAILED,
        });
      });
  };
}
export function changeIdeaDescription(ideaId: string, description: string) {
  return function (dispatch: Dispatch) {
    console.log("change description");
   getAccessToken().then((cookie)=> fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + Cookies.get("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      ideaSetDescription(ideaId:"${ideaId}",description:"${description}") 
    }
    `,
      }),
    }))
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_IDEA_DESCRIPTION_SUCCESS,
            ideaId: ideaId,
            payload: description,
          });
        } else {
          dispatch({
            type: CHANGE_IDEA_DESCRIPTION_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_IDEA_DESCRIPTION_FAILED,
        });
      });
  };
}
export function changeIdeaDetails(ideaId: string, details: string | null) {
  return function (dispatch: Dispatch) {
    console.log("change details");
   getAccessToken().then((cookie)=> fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + Cookies.get("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      ideaSetDetails(ideaId:"${ideaId}",details:${details === null ? "null" : `"${details}"`}) 
    }
    `,
      }),
    }))
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_IDEA_DETAILS_SUCCESS,
            ideaId: ideaId,
            payload: details,
          });
        } else {
          dispatch({
            type: CHANGE_IDEA_DETAILS_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_IDEA_DETAILS_FAILED,
        });
      });
  };
}
export function changeIdeaDirection(ideaId: string, direction : TDirection) {
  return function (dispatch: Dispatch) {
    console.log("change direction");
   getAccessToken().then((cookie)=> fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + Cookies.get("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      ideaSetDirection(ideaId:"${ideaId}",directionId:"${direction.id}") 
    }
    `,
      }),
    }))
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_IDEA_DIRECTION_SUCCESS,
            ideaId: ideaId,
            payload: direction,
          });
        } else {
          dispatch({
            type: CHANGE_IDEA_DIRECTION_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_IDEA_DIRECTION_FAILED,
        });
      });
  };
}

export function changeIdeaGeneration(ideaId: string, generation: string) {
  return function (dispatch: Dispatch) {
    console.log("change category");
   getAccessToken().then((cookie)=> fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + Cookies.get("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      ideaSetGeneration(ideaId:"${ideaId}",generation:${generation.toUpperCase().replaceAll(' ', '_')}) 
    }
    `,
      }),
    }))
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_IDEA_GENERATION_SUCCESS,
            ideaId: ideaId,
            payload: generation,
          });
        } else {
          dispatch({
            type: CHANGE_IDEA_GENERATION_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_IDEA_GENERATION_FAILED,
        });
      });
  };
}

export function deleteIdea(ideaId: string) {
  const date = nowDate();
  return function (dispatch: Dispatch) {
    dispatch({ type: DELETE_IDEA_REQUEST });
    console.log("delete idea");
   getAccessToken().then((cookie)=> fetch(URL, {
      method: "POST",

      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + Cookies.get("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      ideaSetDeletedAt(ideaId:"${ideaId}",deletedAt:"${date}") 
    }
    `,
      }),
    }))
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: DELETE_IDEA_SUCCESS,
            ideaId: ideaId,
            date:date
          });
        } else {
          dispatch({
            type: DELETE_IDEA_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: DELETE_IDEA_FAILED,
        });
      });
  };
}

export function reviewIdea(ideaId: string) {
  const date = nowDate();
  return function (dispatch: Dispatch) {
    console.log("review idea");
   getAccessToken().then((cookie)=> fetch(URL, {
      method: "POST",

      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + Cookies.get("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      ideaSetReviewedAt(ideaId:"${ideaId}",reviewedAt:"${date}") 
    }
    `,
      }),
    }))
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_IDEA_REVIEWED_DATE_SUCCESS,
            ideaId: ideaId,
            date:date
          });
        } else {
          dispatch({
            type: CHANGE_IDEA_REVIEWED_DATE_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_IDEA_REVIEWED_DATE_FAILED,
        });
      });
  };
}
