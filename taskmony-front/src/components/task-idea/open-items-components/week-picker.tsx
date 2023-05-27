import divider from "../../../images/divider.svg";
import { useState } from "react";

type Props = {
  hasBorder?: boolean;
  width?: string;
  value: Array<string>;
  onChange: Function;
  disabled?:boolean;
};
export const WeekPicker = ({ hasBorder, width, value, onChange, disabled }: Props) => {
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
    <div className={"flex justify-between pl-2 pr-2"}>
      <p className={"font-semibold text-sm text-blue-500 pt-0.5"}>repeat on:</p>
      {days.map((day, index) => {
        return (
          <span
            className={`font-semibold text-sm text-blue-500 p-0.5 ml-0.5 cursor-pointer
             ${
              value && value.includes(day.value) ? "underline" : ""
            }
            ${disabled && "text-blue-300"}
            `}
            onClick={() => {
              if (disabled) return;
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
      {hasBorder && <img src={divider} alt=""></img>}
    </div>
  );
};
