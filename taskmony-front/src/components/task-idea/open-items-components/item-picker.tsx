import divider from "../../../images/divider.svg";
import { useState } from "react";

type Props = {
  title: string;
  options: Array<string>;
  option: string;
  hasBorder: boolean;
  onChange: Function;
  width?: string;
  disabled?:boolean;
};
export const ItemPicker = ({
  title,
  options,
  option,
  hasBorder,
  onChange,
  width,
  disabled
}: Props) => {
  const [showMenu, setShowMenu] = useState(false);
  return (
    <div className={"flex justify-between items-center pl-2 relative"}>
      <p className={"font-semibold text-sm text-blue-500"}>{title}:</p>
      <div className={"relative  pl-1 pr-1"}>
        <p
          className={`font-semibold text-sm text-blue-500 cursor-pointer 
          ${disabled && "text-blue-300"}`}
          onClick={() => {if(!disabled) setShowMenu(true)}}
        >
          {option}
        </p>
        {showMenu && (
          <ul className={"absolute top-0 left-0 bg-white pl-1 pr-1 z-50 h-20 overflow-scroll border-solid border-b border-t"}>
            {options.map((option, index) => (
              <li
                className="whitespace-nowrap bg-white z-50 cursor-pointer select-none hover:underline font-semibold text-sm text-blue-500"
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

      {hasBorder && <img src={divider} alt=""></img>}
    </div>
  );
};
