import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import divider from "../../images/divider.svg";
import { useState } from "react";

type Props = {
  title: string;
  options: Array<string>;
  option: string;
  hasBorder: boolean;
};
export const ItemPicker = ({ title, options, option, hasBorder }: Props) => {
  return (
    <div className={"flex justify-between pl-2"}>
      <p className={"font-semibold text-sm text-blue-500"}>{title}:</p>
      <select
        name="options"
        id="options"
        className={"font-semibold text-sm text-blue-500 focus:outline-none"}
      >
        {options.map((o) => (
          <option value={o} className={"font-semibold text-sm text-blue-500"}>
            {o}
          </option>
        ))}
      </select>
      {hasBorder && <img src={divider}></img>}
    </div>
  );
};
