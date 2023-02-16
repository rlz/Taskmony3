import { Dispatch, SetStateAction } from "react";

type CommentProps = {
  text: string;
  author: string;
  time: string;
};

export const Comment = ({ text, author, time }: CommentProps) => {
  const date = new Date(time);
  return (
    <div
      className={`bg-slate-100 m-2 p-2 rounded-lg drop-shadow-sm text-gray-800`}
    >
      <p className={"text-sm p-1 pb-0"}>{text}</p>
      <p className={`text-right text-gray-300 text-sm`}>
        {author}, {date.toLocaleString()}
      </p>
    </div>
  );
};

export const CommentInput = ({ commentValue, changeComment }) => {
  return (
    <div className={`bg-slate-100 m-2 p-2 rounded-lg text-gray-800`}>
      <textarea
        placeholder={"write a comment"}
        className={`text-sm p-1 pb-0 bg-none
              focus:outline-none bg-slate-100
              w-full resize-none
      `}
        value={commentValue}
        onChange={(e) => changeComment(e.target.value)}
      />
    </div>
  );
};
