import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import divider from "../../images/divider.svg";
import { useState } from "react";

type Props = {
  hasBorder?: boolean;
  width?: string;
  value: Array<string>;
  onChange: Function;
};
export const WeekPicker = ({ hasBorder, width, value, onChange }: Props) => {
  const [days, setDays] = useState([
    { name: "M", value: "MONDAY" },
    { name: "T", value: "TUESDAY" },
    { name: "W", value: "WEDNESDAY" },
    { name: "T", value: "THURSDAY" },
    { name: "F", value: "FRIDAY" },
    { name: "S", value: "SATURDAY" },
    { name: "S", value: "SUNDAY" },
  ]);
  return (
    <div className={"flex justify-between pl-2"}>
      <p className={"font-semibold text-sm text-blue-500 pt-0.5"}>repeat on:</p>
      {days.map((day, index) => {
        return (
          <span
            className={`font-semibold text-sm text-blue-500 p-0.5 ml-0.5 cursor-pointer ${
              value && value.includes(day.value) ? "underline" : ""
            }`}
            onClick={() => {
              if (!value) onChange([day.value]);
              else {
                if (value.includes(day.value) && value?.length == 1) return;
                if (value.includes(day.value))
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
