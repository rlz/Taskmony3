import { useMediaQuery } from "react-responsive";
import notifications from "../../../images/notifications.svg";
import notificationsMobile from "../../../images/notifications1.svg";
import filter from "../../../images/filter1.svg";
import { useAppSelector } from "../../../utils/hooks";
import "./badge.css";
import { useEffect, useState } from "react";

type NotificationsBtnPropsT = {
  onClick: Function;
};

export const NotificationsBtn = ({ onClick }: NotificationsBtnPropsT) => {
  const [isOpen, setIsOpen] = useState(false);
  const isSmallScreen = useMediaQuery({ query: "(max-width: 640px)" });
  useEffect(() => {
    if (isOpen) changeFilterWindow();
  }, [isSmallScreen]);
  const changeFilterWindow = () => {
    const filterMenu = document.getElementsByClassName("filter");
    const mainBody = document.getElementsByClassName("mainBody");
    if (isOpen) {
      for (let i = 0; i < filterMenu.length; i++) {
        filterMenu[i].style = "display: none;";
      }
      for (let i = 0; i < mainBody.length; i++) {
        mainBody[i].style = "width: 85vw; display:block;";
      }
    } else {
      for (let i = 0; i < filterMenu.length; i++) {
        filterMenu[i].style = "width: 85vw; display:block;";
      }
      for (let i = 0; i < mainBody.length; i++) {
        mainBody[i].style = "display: none;";
      }
    }
    setIsOpen(!isOpen);
  };
  const digit = useAppSelector((store) => store.notifications.newCount);
  return (
    <div className={"absolute top-0 right-0 m-4 flex gap-2"}>
      {isSmallScreen && (
        <img
          src={filter}
          className={"block m-0 width-50"}
          alt=""
          onClick={changeFilterWindow}
        />
      )}
      <div onClick={(e) => onClick()}>
        <button
          data-after-text={digit}
          data-after-type="badge top right"
          type="button"
        >
          <img
            src={isSmallScreen ? notificationsMobile : notifications}
            className={"block m-0 width-50"}
            alt=""
          />
        </button>
      </div>
    </div>
  );
};
