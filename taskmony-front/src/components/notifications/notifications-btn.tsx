import notifications from "../../images/notifications.svg";
import { useAppSelector } from "../../utils/hooks";
import "./badge.css";

type NotificationsBtnPropsT = {
  onClick: Function;
};

export const NotificationsBtn = ({ onClick }: NotificationsBtnPropsT) => {
  const digit = useAppSelector(store => store.notifications.notifications.length)
  return (
    <div className={"absolute top-0 right-0 m-4"} onClick={(e) => onClick()}>
      <button
        data-after-text={digit}
        data-after-type="badge top right"
        type="button"
      >
        <img src={notifications} className={"block m-0"} />
      </button>
    </div>
  );
};
