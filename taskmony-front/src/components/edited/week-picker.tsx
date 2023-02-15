import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import divider from "../../images/divider.svg";
import { useState } from "react";

type Props = {
  hasBorder?: boolean;
  width?: string;
};
export const WeekPicker = ({ hasBorder, width, value, onChange }: Props) => {
  const [days, setDays] = useState([
    { name: "M", value: "MONDAY", isPicked: false },
    { name: "T", value: "TUESDAY", isPicked: false },
    { name: "W", value: "WEDNESDAY", isPicked: false },
    { name: "T", value: "THURSDAY", isPicked: false },
    { name: "F", value: "FRIDAY", isPicked: false },
    { name: "S", value: "SATURDAY", isPicked: false },
    { name: "S", value: "SUNDAY", isPicked: false },
  ]);
  console.log(value,!value)
  return (
    <div className={"flex justify-between pl-2"}>
      <p className={"font-semibold text-sm text-blue-500 pt-0.5"}>repeat on:</p>
      {days.map((day, index) => {
        return (
          <span
            className={`font-semibold text-sm text-blue-500 p-0.5 ml-0.5 cursor-pointer ${
              (value && value.includes(day.value)) ? "underline" : ""
            }`}
            onClick={() => {
              if (!value) onChange([day.value])
              else {
                if(value.includes(day.value))
                onChange(value.filter((e) => e !== day.value));
                else onChange([...value, day.value]);
              }
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
