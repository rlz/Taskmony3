import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import divider from "../../images/divider.svg";
import { useState } from "react";

type Props = {
  hasBorder?: boolean;
  width?: string;
};
export const WeekPicker = ({ hasBorder, width }: Props) => {
  const [days, setDays] = useState([
    { name: "M", isPicked: false },
    { name: "T", isPicked: false },
    { name: "W", isPicked: false },
    { name: "T", isPicked: false },
    { name: "F", isPicked: false },
    { name: "S", isPicked: false },
    { name: "S", isPicked: false },
  ]);

  return (
    <div className={"flex justify-between pl-2"}>
      <p className={"font-semibold text-sm text-blue-500 pt-0.5"}>repeat on:</p>
      {days.map((day, index) => {
        return (
          <span
            className={`font-semibold text-sm text-blue-500 p-0.5 ml-0.5 cursor-pointer ${
              day.isPicked ? "underline" : ""
            }`}
            onClick={() => {
              const newDays = days.map((day2, index2) => {
                if (index2 == index) {
                  return { ...day2, isPicked: !day2.isPicked };
                } else {
                  return day2;
                }
              });
              setDays(newDays);
            }}
          >
            {day.name}
          </span>
        );
      })}
      {hasBorder && <img src={divider}></img>}
    </div>
  );
};
