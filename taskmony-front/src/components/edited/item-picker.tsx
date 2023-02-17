import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import divider from "../../images/divider.svg";
import { useState } from "react";

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
  return (
    <div className={"flex justify-between items-center pl-2"}>
      <p className={"font-semibold text-sm text-blue-500"}>{title}:</p>
      <select
        name="options"
        id="options"
        defaultValue={option}
        className={
          "font-semibold text-sm text-blue-500 focus:outline-none " + width
        }
        onChange={(e) => onChange(e.target.selectedIndex)}
      >
        {options.map((o,index) => (
          <option value={o} key={index} className={"font-semibold text-sm text-blue-500"}>
            {o}
          </option>
        ))}
      </select>
      {hasBorder && <img src={divider}></img>}
    </div>
  );
};
