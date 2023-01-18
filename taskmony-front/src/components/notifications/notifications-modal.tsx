import { NotificationItem } from "./notification";
import deleteI from "../../images/delete.svg";

type NotificationsModalPropsT = {
  close: Function;
};

export const NotificationsModal = ({ close }: NotificationsModalPropsT) => {
  return (
    <div className="w-1/3 absolute top-0 right-0 m-4 p-3 bg-slate-50 rounded-lg drop-shadow-lg ">
      <img
        src={deleteI}
        className="cursor-pointer mr-0 ml-auto"
        onClick={(e) => close()}
      ></img>
      <NotificationItem
        label={"You was added to “Taskmony”"}
        direction="Taskmony"
        createdBy="Ann Smith"
        time="12:30 1.12.22"
        type='userAdded'
      />
            <NotificationItem
        label={"Sam Green was removed from “Taskmony”"}
        direction="Taskmony"
        createdBy="Ann Smith"
        time="12:30 1.12.22"
        type='userDeleted'
      />
            <NotificationItem
        label={"Task “Create UI for notifications” was added"}
        direction="Taskmony"
        createdBy="Ann Smith"
        time="12:30 1.12.22"
        type='itemAdded'
      />
      <NotificationItem
        label={"Idea “Create UI for notifications” was updated:"}
        direction="Taskmony"
        createdBy="Ann Smith"
        time="12:30 1.12.22"
        details={"generation: hot -> later"}
        type='itemEdited'
      />     
            <NotificationItem
        label={"New comment on task “Create UI for notifications”:"}
        direction="Taskmony"
        createdBy="Ann Smith"
        time="12:30 1.12.22"
        details={"Do we need the date of the notification on the card? Let’s keep..." }
        type='commentAdded'
      />  
                  <NotificationItem
        label={"You have a new task assigned:"}
        direction="Taskmony"
        createdBy="Ann Smith"
        time="12:30 1.12.22"
        details={"Create UI for notifications" }
        type='taskAssigned'
      />    
      <NotificationItem
        label={"Idea “Create UI for notifications” was deleted"}
        direction="Taskmony"
        createdBy="Ann Smith"
        time="12:30 1.12.22"
        type='itemDeleted'
      />
    </div>
  );
};
