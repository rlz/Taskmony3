import ideas from "../../../images/ideas.svg";
import tasks from "../../../images/tasks.svg";
import archive from "../../../images/archive.svg";
import direction from "../../../images/direction.svg";
import arrowUp from "../../../images/arrow-up.svg";
import arrowDown from "../../../images/arrow-down.svg";
import menuClose from "../../../images/menu-close.svg";
import MenuItem from "./menu-item";
import { useEffect, useState } from "react";
import { Profile } from "./profile";
import { ShortMenuItem } from "./short-menu-item";
import { AddBtn2 } from "../buttons/add-btn2";
import { ProfileMenuModal } from "../modals/profile-menu-modal";
import { AddDirectionModal } from "../modals/add-direction-modal";
import { useAppSelector } from "../../../utils/hooks";
import { useMediaQuery } from "react-responsive";

export const SideMenu = () => {
  const [isOpen, setIsOpen] = useState<boolean>(true);
  const isSmallScreen = useMediaQuery({ query: "(max-width: 975px)" });
  const toggleOpen = () => setIsOpen(!isOpen);
  const [isProfileMenuOpen, setIsProfileMenuOpen] = useState<boolean>(false);
  const [openNewDirection, setOpenNewDirection] = useState(false);
  useEffect(() => {
    if (isOpen) setIsOpen(!isSmallScreen);
    if (!isOpen && !isSmallScreen) setIsOpen(true);
  }, [isSmallScreen]);
  return (
    <div
      className={`${
        isOpen && !isSmallScreen ? "w-1/4 lg:w-1/5 xl:w-1/6 " : ""
      } ${
        isOpen && isSmallScreen ? "absolute top-0 left-0 z-30" : ""
      } border-r border-grey-60 h-full bg-slate-50`}
    >
      {openNewDirection && (
        <AddDirectionModal close={() => setOpenNewDirection(false)} />
      )}
      {isProfileMenuOpen && (
        <ProfileMenuModal close={() => setIsProfileMenuOpen(false)} />
      )}
      {isOpen ? (
        <>
          <Profile
            toggleOpen={toggleOpen}
            isOpen={isOpen}
            onClick={() => setIsProfileMenuOpen(true)}
          />{" "}
          <Menu />{" "}
          <DirectionsMenu openDirection={() => setOpenNewDirection(true)} />
        </>
      ) : (
        <SmallMenu toggleOpen={toggleOpen} />
      )}
    </div>
  );
};
type SmallMenuProps = {
  toggleOpen: Function;
};

const SmallMenu = ({ toggleOpen }: SmallMenuProps) => {
  return (
    <div>
      <nav>
        <ul>
          <li>
            <div onClick={() => toggleOpen()}>
              <ShortMenuItem
                to={""}
                name={""}
                icon={menuClose}
                isActive={false}
              />
            </div>
          </li>
          <li>
            <ShortMenuItem
              to={"tasks"}
              name={"My tasks"}
              icon={tasks}
              isActive={false}
            />
          </li>
          <li>
            <ShortMenuItem
              to={"ideas"}
              name={"My ideas"}
              icon={ideas}
              isActive={false}
            />
          </li>
          <li>
            <ShortMenuItem
              to={"archive/tasks"}
              name={"Archive"}
              icon={archive}
              isActive={false}
            />
          </li>
        </ul>
      </nav>
    </div>
  );
};

const Menu = () => {
  return (
    <div>
      <nav>
        <ul>
          <li>
            <MenuItem
              to={"tasks"}
              name={"My tasks"}
              icon={tasks}
              isActive={false}
            />
          </li>
          <li>
            <MenuItem
              to={"ideas"}
              name={"My ideas"}
              icon={ideas}
              isActive={false}
            />
          </li>
          <li>
            <MenuItem
              to={"archive"}
              name={"Archive"}
              icon={archive}
              isActive={false}
            />
          </li>
        </ul>
      </nav>
    </div>
  );
};

type DirectionsMenuPropsT = {
  openDirection: Function;
};

const DirectionsMenu = ({ openDirection }: DirectionsMenuPropsT) => {
  const [isOpen, setIsOpen] = useState<boolean>(true);
  const directions = useAppSelector((store) => store.directions.items).filter(
    (i) => i.deletedAt == null
  );
  const directionsList = directions.map((dir) => (
    <MenuItem
      to={`directions/${dir.id}`}
      key={dir.id}
      name={dir.name}
      icon={direction}
      isActive={false}
    />
  ));
  return (
    <div>
      <div className={"gap-4 flex m-4 justify-between"}>
        <p className={"font-bold uppercase text-gray-300 text-sm"}>
          Directions
        </p>
        <img
          src={isOpen ? arrowUp : arrowDown}
          onClick={() => setIsOpen(!isOpen)}
          alt=""
        ></img>
      </div>
      {isOpen && (
        <nav>
          <ul>{directionsList}</ul>
        </nav>
      )}
      <AddBtn2 label={"add a new direction"} onClick={openDirection} />
    </div>
  );
};
