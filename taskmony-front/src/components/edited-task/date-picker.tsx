import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import divider from "../../images/divider.svg";
import { useState } from "react";

type Props = {
  title: string;
  date: number;
  hasBorder?: boolean;
};
export const DatePicker = ({ title, date, hasBorder }: Props) => {
  return (
    <div className={"flex justify-between pl-2"}>
      <p className={"font-semibold text-sm text-blue-500"}>{title}:</p>
      <input type="date" className="font-semibold text-sm text-blue-500 focus:outline-none w-28" defaultValue={new Date(date).toISOString().substring(0, 10)}/>
      {hasBorder && <img src={divider}></img>}
    </div>
  );
};
