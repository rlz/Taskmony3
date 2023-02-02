import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import divider from "../../images/divider.svg";
import { useState } from "react";

type Props = {
  hasBorder?: boolean;
  width?: string;
};
export const WeekPicker = ({ hasBorder, width }: Props) => {
  const [options,setOptions] = useState([
    ["M", true],
    ["T", false],
    ["W", false],
    ["T", false],
    ["F", false],
    ["S", false],
    ["S", false],
  ]);

  return (
    <div className={"flex justify-between pl-2"}>
      <p className={"font-semibold text-sm text-blue-500 pt-0.5"}>repeat on:</p>
      {options.map((day, index) => {
        return (
          <span
            className={`font-semibold text-sm text-blue-500 p-0.5 ml-0.5 cursor-pointer hover:bg-slate-500 ${
              options[index][1] ? "underline" : ""
            }`}
            onClick={() => {            
              options[index][1] = !options[index][1];

              console.log(options); 
            }}
          >
            {day[0]}
          </span>
        );
      })}
      {hasBorder && <img src={divider}></img>}
    </div>
  );
};
