import notifications from "../../images/notifications.svg";
import './badge.css';

export const NotificationsBtn = () => {
    return (
      <div className={"absolute top-0 right-0 m-4"}>
        	<button data-after-text="2" data-after-type="badge top right" type="button"><img src={notifications} className={"block m-0"}/></button>
      </div>
    );  
  }
