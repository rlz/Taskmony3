import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import menuOpen from "../../images/menu-open.svg";
import { Avatar } from "./avatar";

type ProfileProps = {
  toggleOpen: Function;
  isOpen: boolean;
  onClick: Function
};

export const Profile = ({ toggleOpen, isOpen, onClick }: ProfileProps) => {
  return (
    <div className={"gap-4 flex m-4 mb-8 ml-5 justify-between"}>
      <div className={"gap-6 flex justify-between cursor-pointer"} onClick={() => onClick()}>
        <div>
          <p className={"font-semibold text-sm"}>
            John Doe
          </p>
          <p className={"font-semibold text-sm text-gray-300"}>
            johnd@gmail.com
          </p>
        </div>
      </div>

      <img src={isOpen ? menuOpen : arrowDown} onClick={() => toggleOpen()}></img>
    </div>
  );
};
