import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

async function GetOrganizations(){}

export default function OrganizationPage() {
  return (
    <div className="mt-8 mx-6 mb-6 flex flex-col gap-5">
      <h2 className="text-2xl font-bold">Organizations</h2>
      <hr />
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>ID</TableHead>
            <TableHead>Name</TableHead>
            <TableHead></TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
        </TableBody>
      </Table>
    </div>
  );
}
