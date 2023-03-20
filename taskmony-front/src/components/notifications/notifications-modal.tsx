import { NotificationItem } from "./notification";
import deleteI from "../../images/delete.svg";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import Cookies from 'js-cookie';
import { useState } from "react";
import { RESET_COUNT } from "../../services/actions/notifications";

type NotificationsModalPropsT = {
  close: Function;
};

export const NotificationsModal = ({ close }: NotificationsModalPropsT) => {
  const dispatch = useAppDispatch();
  const notifications = useAppSelector(
    (store) => store.notifications.notifications
  );
  const newCount = useAppSelector((store) => store.notifications.newCount);
  const [showOld, setShowOld] = useState(false);
  const onClose = () => {
    if (!notifications[0]) return;
    const lastNotification = notifications[0];
    Cookies.set("lastNotification", lastNotification.id);
    dispatch({ type: RESET_COUNT });
    console.log(Cookies.get("lastNotification"));
  };

  const notificationsItems = notifications.map((notif) => {
    const date = new Date(notif.modifiedAt);
    let label, type, details;
    if (notif.actionType === "TASK_ASSIGNED") {
      label = "You have a new task assigned:";
      type = "taskAssigned";
      details = notif.name;
    } else if (
      notif.type == "direction" &&
      notif.actionItem?.__typename == "User"
    ) {
      const myId = Cookies.get("id");
      const isYou = notif.actionItem.id === myId;
      label = `${isYou ? "You" : notif.actionItem.displayName} 
      was ${notif.actionType === "ITEM_ADDED" ? "added to" : "removed from"}
       “${notif.direction.name}”`;
      type = notif.actionType === "ITEM_ADDED" ? "userAdded" : "userDeleted";
      details = null;
    } else if (
      notif.type == "direction" &&
      (notif.actionType === "ITEM_ADDED" ||
        notif.actionType === "ITEM_DELETED") &&
      notif.actionItem?.__typename != "User"
    ) {
      label = `${notif.actionItem.__typename} “${
        notif.actionItem.description
      }” was 
      ${notif.actionType === "ITEM_ADDED" ? "added" : "deleted"}`;
      type = notif.actionType === "ITEM_ADDED" ? "itemAdded" : "itemDeleted";
      details = null;
    } else if (
      notif.type == "direction" &&
      notif.actionType === "ITEM_UPDATED"
    ) {
      label = `Direction “${notif.direction.name}” was updated`;
      type = "itemEdited";
      details = `${notif.field.toLowerCase()}:${notif.oldValue}->${
        notif.newValue
      }`;
    } else if (notif.type != "direction" && notif.actionType === "ITEM_ADDED") {
      label = `New comment on ${notif.type} “${notif.name}”:`;
      type = "commentAdded";
      details = notif.actionItem.text;
    } else if (
      notif.type != "direction" &&
      notif.actionType === "ITEM_UPDATED"
    ) {
      label = `${notif.type.charAt(0).toUpperCase() + notif.type.slice(1)} “${
        notif.name
      }” was updated`;
      type = "itemEdited";
      details = `${notif.field.toLowerCase()}:${notif.oldValue}->${
        notif.newValue
      }`;
    } else {
      label = `Some other notification`;
      type = "taskAssigned";
      details = null;
    }
    return (
      <NotificationItem
        label={label}
        key={notif.id}
        direction={notif.direction}
        createdBy={notif.modifiedBy.displayName}
        time={date.toLocaleString()}
        details={details?details:undefined}
        type={type}
      />
    );
  });
  return (
    <div className={`w-1/3 ${newCount < 5 && !showOld?"max-h-3/4" : "h-3/4"} overflow-y-hidden absolute top-0 right-0 m-4 p-3 mt-0 bg-slate-50 rounded-lg drop-shadow-lg `}>
      <img
        src={deleteI}
        className="cursor-pointer mr-0 ml-auto"
        onClick={(e) => {
          onClose();
          close();
        }}
      ></img>
      <div className="h-full w-full overflow-y-scroll overflow-x-hidden">
        {notificationsItems.slice(0, newCount)}
        <p onClick={() => setShowOld(!showOld)} className="cursor-pointer text-center">
          {showOld ? "hide old notifications" : "show old notifications"}
        </p>
        {showOld && notificationsItems.slice(newCount)}
      </div>
    </div>
  );
};
