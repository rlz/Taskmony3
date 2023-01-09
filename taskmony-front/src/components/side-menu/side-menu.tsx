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

export const SideMenu = () => {
  const [isOpen, setIsOpen] = useState<boolean>(true);

  const toggleOpen = () => setIsOpen(!isOpen);
  return (
    <div className={`${isOpen? "w-1/6" : ""} border border-grey-60 h-full`}>
      {isOpen ? (
        <>
          <Profile toggleOpen={toggleOpen} isOpen={isOpen} /> <Menu />{" "}
          <DirectionsMenu />
        </>
      ) : <SmallMenu toggleOpen={toggleOpen}/>}
    </div>
  );
};
type SmallMenuProps = {
  toggleOpen: Function;
};

const SmallMenu = ({toggleOpen} : SmallMenuProps) => {
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

const DirectionsMenu = () => {
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
                to={"directions"}
                name={"Project #1"}
                icon={direction}
                isActive={false}
              />
            </li>
            <li>
              <MenuItem
                to={"directions"}
                name={"Project #1"}
                icon={direction}
                isActive={false}
              />
            </li>
            <li>
              <MenuItem
                to={"directions"}
                name={"Project #1"}
                icon={direction}
                isActive={false}
              />
            </li>
          </ul>
        </nav>
      )}
      <div className={"gap-4 flex m-4"}>
        <img src={addCircle}></img>
        <p className={"font-semibold text-sm text-gray-800"}>
          add a new direction
        </p>
      </div>
    </div>
  );
};
