type BtnProps = {
  label: string;
  onClick: Function;
  style?: string;
  icon?: string;
};

export const AddBtn = ({ label, onClick, style, icon }: BtnProps) => {
  return (
    <div
      className={"gap-1 flex cursor-pointer " + style}
      onClick={() => onClick()}
    >
      <img src={icon}></img>
      <p className={"font-semibold text-sm text-blue-500"}>{label}</p>
    </div>
  );
};
