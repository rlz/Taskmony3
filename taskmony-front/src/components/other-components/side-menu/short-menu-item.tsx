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
          <div className="w-8">
          <img src={icon} alt="" ></img>
          </div>
        </Link>
      ) : (
        <div className="w-8">
        <img src={icon} alt="" className="w-max"></img>
        </div>
      )}
    </div>
  );
};
