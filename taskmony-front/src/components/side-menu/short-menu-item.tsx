import { FC } from "react";
import { Link } from "react-router-dom";

type MenuItemProps = {
  to: string;
  name: string;
  icon: string;
  isActive: boolean;
};

export const ShortMenuItem = ({ to, name, icon, isActive }: MenuItemProps) => {
  return (
    <div className={"gap-4 flex m-4"}>
      {to ? (
        <Link to={to}>
          <img src={icon}></img>
        </Link>
      ) : (
        <img src={icon}></img>
      )}
    </div>
  );
};
