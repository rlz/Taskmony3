import { NotificationItem } from "./notification";
import deleteI from "../../images/delete.svg";
import { useAppSelector } from "../../utils/hooks";
import { getCookie } from "../../utils/cookies";

type NotificationsModalPropsT = {
  close: Function;
};

export const NotificationsModal = ({ close }: NotificationsModalPropsT) => {
  const notifications = useAppSelector(
    (store) => store.notifications.notifications
  );
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
      const myId = getCookie("id");
      const isYou = notif.actionItem.id === myId;
      label = `${isYou ? "You" : notif.actionItem.displayName} 
      was ${notif.actionType === "ITEM_ADDED"?"added to" : "removed from"}
       “${
        notif.direction.name
      }”`;
      type = notif.actionType === "ITEM_ADDED"?"userAdded":"userDeleted";
      details = null;
    } else if (
      notif.type == "direction" &&
      (notif.actionType === "ITEM_ADDED" || notif.actionType === "ITEM_DELETED") &&
      notif.actionItem?.__typename != "User"
    ) {
      label = `${notif.actionItem.__typename} “${notif.actionItem.description}” was 
      ${notif.actionType === "ITEM_ADDED"?"added" : "deleted"}`;
      type = notif.actionType === "ITEM_ADDED"?"itemAdded":"itemDeleted";
      details = null;
    }  
    else if (
      notif.type == "direction" &&
      notif.actionType === "ITEM_UPDATED"
    ) {
      label = `Direction “${notif.direction.name}” was updated`;
      type = "itemEdited";
      details = `${notif.field.toLowerCase()}:${notif.oldValue}->${notif.newValue}`;
    } 
    else if (
      notif.type != "direction" &&
      notif.actionType === "ITEM_ADDED"
    ) {
      label = `New comment on ${notif.type} “${notif.name}”:`;
      type = "commentAdded";
      details = null;
    } 
    else if (
      notif.type != "direction" &&
      notif.actionType === "ITEM_UPDATED"
    ) {
      label = `${notif.type.charAt(0).toUpperCase() + notif.type.slice(1)} “${notif.name}” was updated`;
      type = "itemEdited";
      details = `${notif.field.toLowerCase()}:${notif.oldValue}->${notif.newValue}`;
    } 
    else {
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
        details={details}
        type={type}
      />
    );
  });
  return (
    <div className="w-1/3 h-3/4 overflow-hidden absolute top-0 right-0 m-4 p-3 mt-0 bg-slate-50 rounded-lg drop-shadow-lg ">
      <img
        src={deleteI}
        className="cursor-pointer mr-0 ml-auto"
        onClick={(e) => close()}
      ></img>
      <div className="h-full overflow-scroll" >
      {notificationsItems}
      </div>
    </div>
  );
};
