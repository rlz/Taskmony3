type AvatarProps = {
  name: string;
};

export const Avatar = ({ name }: AvatarProps) => {
  return (
    <div className={"rounded-full bg-sky-100 w-10 h-10"}>
      <span className="mt-2 inline-block align-bottom text-center w-full h-full font-bold uppercase text-gray-600 text-s">
        JD
      </span>
    </div>
  );
};
