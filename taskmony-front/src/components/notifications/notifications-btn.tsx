import notifications from "../../images/notifications.svg";
import "./badge.css";

type NotificationsBtnPropsT = {
  onClick: Function;
};

export const NotificationsBtn = ({ onClick }: NotificationsBtnPropsT) => {
  return (
    <div className={"absolute top-0 right-0 m-4"} onClick={(e) => onClick()}>
      <button
        data-after-text="7"
        data-after-type="badge top right"
        type="button"
      >
        <img src={notifications} className={"block m-0"} />
      </button>
    </div>
  );
};
