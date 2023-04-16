import add from "../../images/add-white.svg";

type BtnProps = {
  label: string;
  onClick: Function;
  unactive?: boolean;
  ref: any;
};

export const BigBtn = ({ label, onClick, unactive, ref }: BtnProps) => {
  return (
    <div
      className={`w-auto inline-block p-1 m-1 mr-2 ${
        unactive === true ? "bg-gray-300" : "bg-blue-500 cursor-pointer"
      } rounded-lg `}
      onClick={(e) => onClick()}
      ref={ref}
    >
      <span className={"text-white p-1 pr-2 pl-2"}>{label}</span>
    </div>
  );
};
