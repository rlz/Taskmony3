import { Link } from "react-router-dom";
import ideas from "../../images/ideas.svg";
import tasks from "../../images/tasks.svg";
import archive from "../../images/archive.svg";
import direction from "../../images/direction.svg";
import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import addCircle from "../../images/add_circle.svg";
import menuOpen from "../../images/menu-open.svg";
import menuClose from "../../images/menu-close.svg";
import MenuItem from "./menu-item";
import { useState } from "react";
import { Profile } from "./profile";
import { ShortMenuItem } from "./short-menu-item";
import { AddBtn2 } from "../add-btn/add-btn2";
import { ProfileMenuModal } from "../profile-menu-modal/profile-menu-modal";
import { AddDirectionModal } from "../add-direction-modal/add-direction-modal";

export const SideMenu = () => {
  const [isOpen, setIsOpen] = useState<boolean>(true);
  const toggleOpen = () => setIsOpen(!isOpen);
  const [isProfileMenuOpen, setIsProfileMenuOpen] = useState<boolean>(false);
  const [openNewDirection, setOpenNewDirection] = useState(false);
  return (
    <div className={`${isOpen ? "w-1/6" : ""} border border-grey-60 h-full`}>
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

type DirectionsMenuPropsT = {
  openDirection: Function;
};

const DirectionsMenu = ({ openDirection }: DirectionsMenuPropsT) => {
  const [isOpen, setIsOpen] = useState<boolean>(true);

  return (
    <div>
      <div className={"gap-4 flex m-4 justify-between"}>
        <p className={"font-bold uppercase text-gray-300 text-sm"}>
          Directions
        </p>
        <img
          src={isOpen ? arrowUp : arrowDown}
          onClick={() => setIsOpen(!isOpen)}
        ></img>
      </div>
      {isOpen && (
        <nav>
          <ul>
            <li>
              <MenuItem
                to={"directions/1/tasks"}
                name={"Project #1"}
                icon={direction}
                isActive={false}
              />
            </li>
            <li>
              <MenuItem
                to={"directions/2/tasks"}
                name={"Project #1"}
                icon={direction}
                isActive={false}
              />
            </li>
            <li>
              <MenuItem
                to={"directions/3/tasks"}
                name={"Project #1"}
                icon={direction}
                isActive={false}
              />
            </li>
          </ul>
        </nav>
      )}
      <AddBtn2 label={"add a new direction"} onClick={openDirection} />
    </div>
  );
};
