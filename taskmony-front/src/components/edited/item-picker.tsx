import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import divider from "../../images/divider.svg";
import { useState } from "react";
import { Select } from "./select";

type Props = {
  title: string;
  options: Array<string>;
  option: string;
  hasBorder: boolean;
  onChange: Function;
  width?: string;
};
export const ItemPicker = ({
  title,
  options,
  option,
  hasBorder,
  onChange,
  width,
}: Props) => {
  const [showMenu, setShowMenu] = useState(false);
  return (
    <div className={"flex justify-between items-center pl-2 relative"}>
      <p className={"font-semibold text-sm text-blue-500"}>{title}:</p>
      <div className={"relative  pl-1 pr-1"}>
        <p
          className={"font-semibold text-sm text-blue-500 cursor-pointer"}
          onClick={() => setShowMenu(true)}
        >
          {option}
        </p>
        {showMenu && (
          <ul className={"absolute top-0 left-0 bg-white pl-1 pr-1 z-50 h-20 overflow-scroll"}>
            {options.map((option, index) => (
              <li
                className="bg-white z-50 cursor-pointer select-none hover:underline font-semibold text-sm text-blue-500"
                onClick={(e) => {
                  setShowMenu(false);
                  onChange(index);
                }}
                key={index}
              >
                {option}
              </li>
            ))}
          </ul>
        )}
      </div>

      {hasBorder && <img src={divider}></img>}
    </div>
  );
};
