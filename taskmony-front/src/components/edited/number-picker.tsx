import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import divider from "../../images/divider.svg";
import { useState } from "react";

type Props = {
  title: string;
  min: number;
  max: number;
  after: string;
  hasBorder?: boolean;
};
export const NumberPicker = ({ title, min, max, hasBorder, after }: Props) => {
  return (
    <div className={"flex justify-between pl-2"}>
      <p className={"font-semibold text-sm text-blue-500 pr-1"}>{title}</p>
      <input type="number" className="font-semibold text-sm text-blue-500 focus:outline-none w-8" 
      defaultValue={min}
      min={min} max={max} />
     <p className={"font-semibold text-sm text-blue-500 pr-2 whitespace-nowrap"}>{after} </p>
      {hasBorder && <img src={divider}></img>}
    </div>
  );
};