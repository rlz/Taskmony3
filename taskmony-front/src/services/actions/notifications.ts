import { Dispatch } from "redux";
import { checkResponse } from "../../utils/APIUtils";
import { getCookie } from "../../utils/cookies";
import { BASE_URL } from "../../utils/data";
import { notificationsAllQuery } from "../../utils/queries";
export const GET_NOTIFICATIONS_REQUEST = "GET_NOTIFICATIONS_REQUEST";
export const GET_NOTIFICATIONS_SUCCESS = "GET_NOTIFICATIONS_SUCCESS";
export const GET_NOTIFICATIONS_FAILED = "GET_NOTIFICATIONS_FAILED";

export const RESET_COUNT = "RESET_COUNT";

const URL = BASE_URL + "/graphql";

export function getNotifications() {
  return function (dispatch: Dispatch) {
    dispatch({ type: GET_NOTIFICATIONS_REQUEST });
    console.log("getting notifications");
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `{
            tasks{
            id
            description
            direction {
                name
                id
            }
            notifications{
                ${notificationsAllQuery}
            }
          }
            ideas{
            id
            description
            direction {
                name
                id
            }
            notifications{
                ${notificationsAllQuery}
            }
          }
          directions{
            id
            name
            notifications{
                ${notificationsAllQuery}
            }
          }
        }`,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          const tasksNotifications = res.data.tasks
            .filter((n) => n.notifications.length)
            .map((t) => {
              return t.notifications.map((n) => {
                return { type: "task", direction:t.direction, name: t.description, id: t.id, ...n };
              });
            }).flat();
            const ideasNotifications = res.data.ideas
            .filter((n) => n.notifications.length)
            .map((t) => {
              return t.notifications.map((n) => {
                return { type: "idea",direction:t.direction, name: t.description, id: t.id, ...n };
              });
            }).flat();
            const directionsNotifications = res.data.directions
            .filter((n) => n.notifications.length)
            .map((t) => {
              return t.notifications.map((n) => {
                return { type: "direction", direction:{name:t.name,id:t.id},name: t.name, id: t.id, ...n };
              });
            }).flat();
          const notifications = [...tasksNotifications,...directionsNotifications,...ideasNotifications].sort((a,b) => (a.modifiedAt < b.modifiedAt) ? 1 : ((b.modifiedAt < a.modifiedAt) ? -1 : 0))
          console.log(notifications);
          dispatch({
            type: GET_NOTIFICATIONS_SUCCESS,
            notifications: notifications,
          });
        } else {
          dispatch({
            type: GET_NOTIFICATIONS_FAILED,
          });
        }
      })
      .catch((error) => {
        console.log(error)
        dispatch({
          type: GET_NOTIFICATIONS_FAILED,
        });
      });
  };
}
