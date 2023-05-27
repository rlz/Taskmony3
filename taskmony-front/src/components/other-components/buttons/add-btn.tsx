import add from "../../../images/add-white.svg";

type BtnProps = {
  label: string;
  onClick: Function;
  unactive?: boolean;
};

export const AddBtn = ({ label, onClick, unactive }: BtnProps) => {
  return (
    <div
      className={`gap-4 flex p-2 mt-4 mb-2 w-full justify-center ${
        unactive ? "bg-gray-300" : "bg-blue-500 cursor-pointer"
      } rounded-lg `}
      onClick={() => {
        if (!unactive) onClick();
      }}
    >
      <img src={add} alt=""></img>
      <span className={"text-white"}>{label}</span>
    </div>
  );
};
